//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/admin_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'approval_limit_options_response.g.dart';

/// ApprovalLimitOptionsResponse
///
/// Properties:
/// * [roles] 
/// * [branches] 
/// * [documentTypes] 
/// * [currencies] 
@BuiltValue()
abstract class ApprovalLimitOptionsResponse implements Built<ApprovalLimitOptionsResponse, ApprovalLimitOptionsResponseBuilder> {
  @BuiltValueField(wireName: r'roles')
  BuiltList<AdminRefResponse>? get roles;

  @BuiltValueField(wireName: r'branches')
  BuiltList<AdminRefResponse>? get branches;

  @BuiltValueField(wireName: r'documentTypes')
  BuiltList<String>? get documentTypes;

  @BuiltValueField(wireName: r'currencies')
  BuiltList<String>? get currencies;

  ApprovalLimitOptionsResponse._();

  factory ApprovalLimitOptionsResponse([void updates(ApprovalLimitOptionsResponseBuilder b)]) = _$ApprovalLimitOptionsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ApprovalLimitOptionsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ApprovalLimitOptionsResponse> get serializer => _$ApprovalLimitOptionsResponseSerializer();
}

class _$ApprovalLimitOptionsResponseSerializer implements PrimitiveSerializer<ApprovalLimitOptionsResponse> {
  @override
  final Iterable<Type> types = const [ApprovalLimitOptionsResponse, _$ApprovalLimitOptionsResponse];

  @override
  final String wireName = r'ApprovalLimitOptionsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ApprovalLimitOptionsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.roles != null) {
      yield r'roles';
      yield serializers.serialize(
        object.roles,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRefResponse)]),
      );
    }
    if (object.branches != null) {
      yield r'branches';
      yield serializers.serialize(
        object.branches,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRefResponse)]),
      );
    }
    if (object.documentTypes != null) {
      yield r'documentTypes';
      yield serializers.serialize(
        object.documentTypes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.currencies != null) {
      yield r'currencies';
      yield serializers.serialize(
        object.currencies,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ApprovalLimitOptionsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ApprovalLimitOptionsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'roles':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRefResponse)]),
          ) as BuiltList<AdminRefResponse>?;
          if (valueDes == null) continue;
          result.roles.replace(valueDes);
          break;
        case r'branches':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRefResponse)]),
          ) as BuiltList<AdminRefResponse>?;
          if (valueDes == null) continue;
          result.branches.replace(valueDes);
          break;
        case r'documentTypes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.documentTypes.replace(valueDes);
          break;
        case r'currencies':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.currencies.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ApprovalLimitOptionsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ApprovalLimitOptionsResponseBuilder();
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


