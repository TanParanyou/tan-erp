//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_routing_step_request.g.dart';

/// BomRoutingStepRequest
///
/// Properties:
/// * [sequence] 
/// * [operationCode] 
/// * [operationName] 
/// * [workCenterId] 
/// * [setupHours] 
/// * [runHours] 
/// * [isSubcontracted] 
/// * [stdSubcontractCost] 
/// * [note] 
@BuiltValue()
abstract class BomRoutingStepRequest implements Built<BomRoutingStepRequest, BomRoutingStepRequestBuilder> {
  @BuiltValueField(wireName: r'sequence')
  int? get sequence;

  @BuiltValueField(wireName: r'operationCode')
  String? get operationCode;

  @BuiltValueField(wireName: r'operationName')
  String? get operationName;

  @BuiltValueField(wireName: r'workCenterId')
  String? get workCenterId;

  @BuiltValueField(wireName: r'setupHours')
  double? get setupHours;

  @BuiltValueField(wireName: r'runHours')
  double? get runHours;

  @BuiltValueField(wireName: r'isSubcontracted')
  bool? get isSubcontracted;

  @BuiltValueField(wireName: r'stdSubcontractCost')
  double? get stdSubcontractCost;

  @BuiltValueField(wireName: r'note')
  String? get note;

  BomRoutingStepRequest._();

  factory BomRoutingStepRequest([void updates(BomRoutingStepRequestBuilder b)]) = _$BomRoutingStepRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomRoutingStepRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomRoutingStepRequest> get serializer => _$BomRoutingStepRequestSerializer();
}

class _$BomRoutingStepRequestSerializer implements PrimitiveSerializer<BomRoutingStepRequest> {
  @override
  final Iterable<Type> types = const [BomRoutingStepRequest, _$BomRoutingStepRequest];

  @override
  final String wireName = r'BomRoutingStepRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomRoutingStepRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sequence != null) {
      yield r'sequence';
      yield serializers.serialize(
        object.sequence,
        specifiedType: const FullType(int),
      );
    }
    if (object.operationCode != null) {
      yield r'operationCode';
      yield serializers.serialize(
        object.operationCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.operationName != null) {
      yield r'operationName';
      yield serializers.serialize(
        object.operationName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workCenterId != null) {
      yield r'workCenterId';
      yield serializers.serialize(
        object.workCenterId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.setupHours != null) {
      yield r'setupHours';
      yield serializers.serialize(
        object.setupHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.runHours != null) {
      yield r'runHours';
      yield serializers.serialize(
        object.runHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.isSubcontracted != null) {
      yield r'isSubcontracted';
      yield serializers.serialize(
        object.isSubcontracted,
        specifiedType: const FullType(bool),
      );
    }
    if (object.stdSubcontractCost != null) {
      yield r'stdSubcontractCost';
      yield serializers.serialize(
        object.stdSubcontractCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomRoutingStepRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomRoutingStepRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sequence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sequence = valueDes;
          break;
        case r'operationCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.operationCode = valueDes;
          break;
        case r'operationName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.operationName = valueDes;
          break;
        case r'workCenterId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workCenterId = valueDes;
          break;
        case r'setupHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.setupHours = valueDes;
          break;
        case r'runHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.runHours = valueDes;
          break;
        case r'isSubcontracted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isSubcontracted = valueDes;
          break;
        case r'stdSubcontractCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.stdSubcontractCost = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomRoutingStepRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomRoutingStepRequestBuilder();
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


