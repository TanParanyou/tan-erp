//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'reassign_opportunity_owner_request.g.dart';

/// ReassignOpportunityOwnerRequest
///
/// Properties:
/// * [targetOwnerUserId] 
/// * [expectedVersion] 
@BuiltValue()
abstract class ReassignOpportunityOwnerRequest implements Built<ReassignOpportunityOwnerRequest, ReassignOpportunityOwnerRequestBuilder> {
  @BuiltValueField(wireName: r'targetOwnerUserId')
  String? get targetOwnerUserId;

  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  ReassignOpportunityOwnerRequest._();

  factory ReassignOpportunityOwnerRequest([void updates(ReassignOpportunityOwnerRequestBuilder b)]) = _$ReassignOpportunityOwnerRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReassignOpportunityOwnerRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReassignOpportunityOwnerRequest> get serializer => _$ReassignOpportunityOwnerRequestSerializer();
}

class _$ReassignOpportunityOwnerRequestSerializer implements PrimitiveSerializer<ReassignOpportunityOwnerRequest> {
  @override
  final Iterable<Type> types = const [ReassignOpportunityOwnerRequest, _$ReassignOpportunityOwnerRequest];

  @override
  final String wireName = r'ReassignOpportunityOwnerRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReassignOpportunityOwnerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.targetOwnerUserId != null) {
      yield r'targetOwnerUserId';
      yield serializers.serialize(
        object.targetOwnerUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.expectedVersion != null) {
      yield r'expectedVersion';
      yield serializers.serialize(
        object.expectedVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReassignOpportunityOwnerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReassignOpportunityOwnerRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'targetOwnerUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.targetOwnerUserId = valueDes;
          break;
        case r'expectedVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReassignOpportunityOwnerRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReassignOpportunityOwnerRequestBuilder();
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


