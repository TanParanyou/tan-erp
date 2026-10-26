//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/service_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'defect_response.g.dart';

/// DefectResponse
///
/// Properties:
/// * [id] 
/// * [no] 
/// * [description] 
/// * [severity] 
/// * [status] 
/// * [reportedBy] 
/// * [reportedAtUtc] 
/// * [resolvedBy] 
/// * [resolvedAtUtc] 
/// * [resolutionNote] 
/// * [verifiedBy] 
/// * [verifiedAtUtc] 
/// * [reopenCount] 
/// * [lastReopenReason] 
@BuiltValue()
abstract class DefectResponse implements Built<DefectResponse, DefectResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'no')
  int? get no;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'severity')
  String? get severity;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'reportedBy')
  ServicePersonResponse? get reportedBy;

  @BuiltValueField(wireName: r'reportedAtUtc')
  DateTime? get reportedAtUtc;

  @BuiltValueField(wireName: r'resolvedBy')
  ServicePersonResponse? get resolvedBy;

  @BuiltValueField(wireName: r'resolvedAtUtc')
  DateTime? get resolvedAtUtc;

  @BuiltValueField(wireName: r'resolutionNote')
  String? get resolutionNote;

  @BuiltValueField(wireName: r'verifiedBy')
  ServicePersonResponse? get verifiedBy;

  @BuiltValueField(wireName: r'verifiedAtUtc')
  DateTime? get verifiedAtUtc;

  @BuiltValueField(wireName: r'reopenCount')
  int? get reopenCount;

  @BuiltValueField(wireName: r'lastReopenReason')
  String? get lastReopenReason;

  DefectResponse._();

  factory DefectResponse([void updates(DefectResponseBuilder b)]) = _$DefectResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DefectResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DefectResponse> get serializer => _$DefectResponseSerializer();
}

class _$DefectResponseSerializer implements PrimitiveSerializer<DefectResponse> {
  @override
  final Iterable<Type> types = const [DefectResponse, _$DefectResponse];

  @override
  final String wireName = r'DefectResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DefectResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.no != null) {
      yield r'no';
      yield serializers.serialize(
        object.no,
        specifiedType: const FullType(int),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.severity != null) {
      yield r'severity';
      yield serializers.serialize(
        object.severity,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reportedBy != null) {
      yield r'reportedBy';
      yield serializers.serialize(
        object.reportedBy,
        specifiedType: const FullType(ServicePersonResponse),
      );
    }
    if (object.reportedAtUtc != null) {
      yield r'reportedAtUtc';
      yield serializers.serialize(
        object.reportedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.resolvedBy != null) {
      yield r'resolvedBy';
      yield serializers.serialize(
        object.resolvedBy,
        specifiedType: const FullType(ServicePersonResponse),
      );
    }
    if (object.resolvedAtUtc != null) {
      yield r'resolvedAtUtc';
      yield serializers.serialize(
        object.resolvedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.resolutionNote != null) {
      yield r'resolutionNote';
      yield serializers.serialize(
        object.resolutionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.verifiedBy != null) {
      yield r'verifiedBy';
      yield serializers.serialize(
        object.verifiedBy,
        specifiedType: const FullType(ServicePersonResponse),
      );
    }
    if (object.verifiedAtUtc != null) {
      yield r'verifiedAtUtc';
      yield serializers.serialize(
        object.verifiedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.reopenCount != null) {
      yield r'reopenCount';
      yield serializers.serialize(
        object.reopenCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.lastReopenReason != null) {
      yield r'lastReopenReason';
      yield serializers.serialize(
        object.lastReopenReason,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    DefectResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DefectResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'no':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.no = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'severity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.severity = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'reportedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.reportedBy.replace(valueDes);
          break;
        case r'reportedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.reportedAtUtc = valueDes;
          break;
        case r'resolvedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.resolvedBy.replace(valueDes);
          break;
        case r'resolvedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.resolvedAtUtc = valueDes;
          break;
        case r'resolutionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resolutionNote = valueDes;
          break;
        case r'verifiedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.verifiedBy.replace(valueDes);
          break;
        case r'verifiedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.verifiedAtUtc = valueDes;
          break;
        case r'reopenCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.reopenCount = valueDes;
          break;
        case r'lastReopenReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lastReopenReason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  DefectResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DefectResponseBuilder();
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


