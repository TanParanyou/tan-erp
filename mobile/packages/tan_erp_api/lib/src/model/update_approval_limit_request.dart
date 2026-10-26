//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_approval_limit_request.g.dart';

/// UpdateApprovalLimitRequest
///
/// Properties:
/// * [isUnlimited] 
/// * [maxAmount] 
/// * [isActive] 
@BuiltValue()
abstract class UpdateApprovalLimitRequest implements Built<UpdateApprovalLimitRequest, UpdateApprovalLimitRequestBuilder> {
  @BuiltValueField(wireName: r'isUnlimited')
  bool? get isUnlimited;

  @BuiltValueField(wireName: r'maxAmount')
  double? get maxAmount;

  @BuiltValueField(wireName: r'isActive')
  bool? get isActive;

  UpdateApprovalLimitRequest._();

  factory UpdateApprovalLimitRequest([void updates(UpdateApprovalLimitRequestBuilder b)]) = _$UpdateApprovalLimitRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateApprovalLimitRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateApprovalLimitRequest> get serializer => _$UpdateApprovalLimitRequestSerializer();
}

class _$UpdateApprovalLimitRequestSerializer implements PrimitiveSerializer<UpdateApprovalLimitRequest> {
  @override
  final Iterable<Type> types = const [UpdateApprovalLimitRequest, _$UpdateApprovalLimitRequest];

  @override
  final String wireName = r'UpdateApprovalLimitRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateApprovalLimitRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.isUnlimited != null) {
      yield r'isUnlimited';
      yield serializers.serialize(
        object.isUnlimited,
        specifiedType: const FullType.nullable(bool),
      );
    }
    if (object.maxAmount != null) {
      yield r'maxAmount';
      yield serializers.serialize(
        object.maxAmount,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.isActive != null) {
      yield r'isActive';
      yield serializers.serialize(
        object.isActive,
        specifiedType: const FullType.nullable(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateApprovalLimitRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateApprovalLimitRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'isUnlimited':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isUnlimited = valueDes;
          break;
        case r'maxAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.maxAmount = valueDes;
          break;
        case r'isActive':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isActive = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateApprovalLimitRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateApprovalLimitRequestBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


