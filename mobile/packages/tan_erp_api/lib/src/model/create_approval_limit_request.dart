//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_approval_limit_request.g.dart';

/// CreateApprovalLimitRequest
///
/// Properties:
/// * [roleId] 
/// * [documentType] 
/// * [branchId] 
/// * [currency] 
/// * [isUnlimited] 
/// * [maxAmount] 
@BuiltValue()
abstract class CreateApprovalLimitRequest implements Built<CreateApprovalLimitRequest, CreateApprovalLimitRequestBuilder> {
  @BuiltValueField(wireName: r'roleId')
  String? get roleId;

  @BuiltValueField(wireName: r'documentType')
  String? get documentType;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'isUnlimited')
  bool? get isUnlimited;

  @BuiltValueField(wireName: r'maxAmount')
  double? get maxAmount;

  CreateApprovalLimitRequest._();

  factory CreateApprovalLimitRequest([void updates(CreateApprovalLimitRequestBuilder b)]) = _$CreateApprovalLimitRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateApprovalLimitRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateApprovalLimitRequest> get serializer => _$CreateApprovalLimitRequestSerializer();
}

class _$CreateApprovalLimitRequestSerializer implements PrimitiveSerializer<CreateApprovalLimitRequest> {
  @override
  final Iterable<Type> types = const [CreateApprovalLimitRequest, _$CreateApprovalLimitRequest];

  @override
  final String wireName = r'CreateApprovalLimitRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateApprovalLimitRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.roleId != null) {
      yield r'roleId';
      yield serializers.serialize(
        object.roleId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.documentType != null) {
      yield r'documentType';
      yield serializers.serialize(
        object.documentType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
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
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateApprovalLimitRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateApprovalLimitRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'roleId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roleId = valueDes;
          break;
        case r'documentType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.documentType = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateApprovalLimitRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateApprovalLimitRequestBuilder();
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


